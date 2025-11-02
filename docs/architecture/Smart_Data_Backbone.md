# سند طراحی فنی: زیرساخت داده‌های هوشمند (Intelligent Data Backbone)

## ۱. اصول راهنما (Guiding Principles)

- **یکپارچگی داده (Data Integrity):** داده‌های تراکنشی باید کاملاً سازگار (Consistent) و قابل اعتماد باقی بمانند.
- **جستجوی سریع معنایی (Fast Semantic Search):** سیستم باید قادر به انجام جستجوهای شباهت (Similarity Searches) بر روی حجم عظیمی از داده‌های متنی با تاخیر کم (Low Latency) باشد.
- **معماری آماده برای ML:** زیرساخت باید به گونه‌ای طراحی شود که در آینده به راحتی با پایپ‌لاین‌های یادگیری ماشین (ML Pipelines) برای آموزش مدل‌های سفارشی ادغام شود.
- **هزینه-اثربخشی (Cost-Effectiveness):** راهکار انتخابی باید تعادل مناسبی بین عملکرد و هزینه‌های عملیاتی برقرار کند.

## ۲. طراحی شمای PostgreSQL برای داده‌های رابطه‌ای و بُرداری

### ۲.۱. استفاده از `pgvector`
برای فعال‌سازی قابلیت‌های جستجوی بُرداری، افزونه `pgvector` را به نمونه PostgreSQL موجود اضافه خواهیم کرد. این رویکرد به ما اجازه می‌دهد تا بدون اضافه کردن یک پایگاه داده جدید، از قابلیت‌های جستجوی معنایی بهره‌مند شویم.

### ۲.۲. طراحی جدول `Embeddings`
یک جدول جدید به نام `Embeddings` برای ذخیره بردارهای متنی ایجاد می‌کنیم.

```sql
CREATE TABLE "Embeddings" (
    "Id" UUID PRIMARY KEY,
    "SourceEntityId" UUID NOT NULL,
    "SourceEntityType" TEXT NOT NULL,
    "UserId" UUID REFERENCES "Users"("Id"),
    "EmbeddingVector" vector(1536) NOT NULL,
    "ContentHash" TEXT NOT NULL,
    "ModelVersion" TEXT NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

- **`EmbeddingVector`**: این ستون بردار تولید شده توسط مدل `text-embedding-3-small` OpenAI را ذخیره می‌کند.
- **`ContentHash`**: هش SHA256 محتوای منبع برای جلوگیری از پردازش مجدد و تکراری محتواها استفاده می‌شود.
- **`ModelVersion`**: نسخه مدل Embedding برای مدیریت به‌روزرسانی‌های آینده ذخیره می‌شود.

### ۲.۳. استراتژی ایندکس‌گذاری
- **ایندکس HNSW:** یک ایندکس `HNSW (Hierarchical Navigable Small World)` بر روی ستون `EmbeddingVector` ایجاد می‌کنیم.
  ```sql
  CREATE INDEX ON "Embeddings" USING hnsw ("EmbeddingVector" vector_cosine_ops);
  ```
  **توجیه:** HNSW تعادل بسیار خوبی بین سرعت جستجو، دقت (recall) و مصرف حافظه ارائه می‌دهد و برای حجم داده‌های متوسط تا بزرگ، انتخاب بهتری نسبت به IVF است.

- **ایندکس‌های ترکیبی:** برای بهینه‌سازی فیلترینگ، ایندکس‌های B-Tree زیر را اضافه می‌کنیم:
  ```sql
  CREATE INDEX ON "Embeddings" ("SourceEntityType", "SourceEntityId");
  CREATE INDEX ON "Embeddings" ("UserId");
  ```

## ۳. پایپ‌لاین تولید و غنی‌سازی Embedding

این پایپ‌لاین به صورت **غیرهمزمان** و **رویداد-محور** طراحی می‌شود.

1.  **تعریف رویداد:** یک رویداد جدید به نام `ContentCreatedForEmbeddingEvent` تعریف می‌شود.
    ```json
    {
      "EntityId": "uuid",
      "EntityType": "journal | course",
      "Content": "متن محتوا...",
      "UserId": "uuid"
    }
    ```
2.  **پابلیش رویداد:** میکروسرویس‌های `IdentityService` (برای ژورنال‌ها) و `CoachingService` (برای دوره‌ها) پس از ایجاد محتوای جدید، این رویداد را در RabbitMQ پابلیش می‌کنند.
3.  **میکروسرویس `EmbeddingService`:**
    - یک میکروسرویس جدید و اختصاصی به نام `EmbeddingService` ایجاد می‌شود.
    - این سرویس به رویداد `ContentCreatedForEmbeddingEvent` گوش می‌دهد.
    - با دریافت رویداد، API امبدینگ OpenAI را فراخوانی می‌کند (با مدیریت خطا و Retry Policy).
    - `ContentHash` را محاسبه کرده و نتیجه را در جدول `Embeddings` ذخیره می‌کند.

## ۴. طراحی API برای موتور پیشنهاددهی

1.  **میکروسرویس `RecommendationService`:** یک میکروسرویس جدید برای مدیریت منطق پیشنهاددهی ایجاد می‌شود.
2.  **Endpoint:** `GET /api/recommendations/for-journal/{journalId}`
3.  **منطق داخلی:**
    - با دریافت `journalId`، ابتدا بردار (Embedding) مربوط به آن را از جدول `Embeddings` پیدا می‌کند.
    - یک جستجوی شباهت کسینوسی (k-NN) روی جدول `Embeddings` انجام می‌دهد تا `N` آیتم مشابه (مثلاً ۵ دوره آموزشی مرتبط) را پیدا کند.
      ```sql
      SELECT "SourceEntityId"
      FROM "Embeddings"
      WHERE "SourceEntityType" = 'course'
      ORDER BY "EmbeddingVector" <=> (
          SELECT "EmbeddingVector" FROM "Embeddings" WHERE "SourceEntityId" = '{journalId}' AND "SourceEntityType" = 'journal'
      )
      LIMIT 5;
      ```
    - شناسه‌های دوره‌های پیشنهادی را به `CoachingService` ارسال کرده و اطلاعات کامل آن‌ها را برای نمایش به کاربر دریافت می‌کند.

## ۵. تحلیل مقیاس‌پذیری و Tradeoffs

- **چه زمانی مهاجرت کنیم؟** زمانی که تعداد بردارها از **۱۰ میلیون** فراتر رود یا زمانی که نیاز به قابلیت‌های پیشرفته‌تری مانند فیلترینگ بی‌درنگ (real-time filtering) یا مقیاس‌پذیری افقی پیچیده داشته باشیم، باید به یک Vector Database اختصاصی مانند **Pinecone** یا **Weaviate** مهاجرت کنیم.
- **معیارها:** تاخیر جستجو (Query Latency)، توان عملیاتی (Throughput)، هزینه‌های زیرساخت، و پیچیدگی عملیاتی.
