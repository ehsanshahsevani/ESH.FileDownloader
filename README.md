
# ESH File Downloader 📥

این کتابخانه برای دانلود و ذخیره‌سازی انواع فایل‌ها از URL در سرور طراحی شده است. از آن می‌توانید برای مدیریت و دانلود انواع فایل‌ها مانند تصاویر، پی‌دی‌اف، ویدئو و موسیقی استفاده کنید. فایل‌ها به صورت خودکار در پوشه‌ای که تعیین می‌کنید ذخیره خواهند شد و نام جدید (GUID) به آن‌ها اختصاص داده می‌شود.

## نصب 📦

برای نصب این کتابخانه در پروژه خود، می‌توانید از دستور زیر استفاده کنید:

```bash
dotnet add package ESH.FileDownloader
```

## نحوه استفاده 🛠️

### 1. دانلود و ذخیره‌سازی فایل‌ها

برای دانلود فایل‌ها از URL و ذخیره آن‌ها در سرور، از متد `DownloadAndSaveFilesAsync` استفاده کنید:

```csharp
using ESH.FileDownloader;
using System.Collections.Generic;

var fileUrls = new List<string>
{
    "https://example.com/image1.jpg",
    "https://example.com/document.pdf"
};

var fileType = FileType.Images;  // یا FileType.Pdf برای پی‌دی‌اف، FileType.Video و غیره
var saveDirectory = "C:\Files";

var result = await Downloader.DownloadAndSaveFilesAsync(fileUrls, fileType, saveDirectory);

foreach (var file in result)
{
    if (file.HasError)
    {
        Console.WriteLine($"Error: {file.Error}");
    }
    else
    {
        Console.WriteLine($"File {file.OriginalName} saved as {file.GuidName}");
    }
}
```

### 2. حذف فایل‌ها

برای حذف فایل‌هایی که قبلاً ذخیره کرده‌اید، می‌توانید از متد `DeleteFiles` استفاده کنید:

```csharp
using ESH.FileDownloader;
using System.Collections.Generic;

var fileNamesToDelete = new List<string> { "file1.jpg", "file2.pdf" };
var folderPath = "C:\Files";

Downloader.DeleteFiles(fileNamesToDelete, folderPath);
```

## توضیحات 📋

- **مدیریت پسوند و اندازه فایل**: این کتابخانه به صورت خودکار پسوند فایل‌ها را بررسی کرده و اندازه آن‌ها را مطابق با نوع فایل (تصویر، پی‌دی‌اف، ویدئو و موسیقی) محدود می‌کند.
  
  - تصاویر: پسوندهای `.jpg`, `.jpeg`, `.png`, `.gif` و حداکثر حجم 1MB
  - پی‌دی‌اف: پسوند `.pdf` و حداکثر حجم 20MB
  - ویدئو: پسوندهای `.mp4`, `.avi`, `.mkv`, `.mov` و حداکثر حجم 3MB
  - موسیقی: پسوندهای `.mp3`, `.wav`, `.flac` و حداکثر حجم 3MB

- **مدیریت خطا**: در صورتی که خطایی در هنگام دانلود یا ذخیره‌سازی رخ دهد، فایل‌های قبلاً ذخیره‌شده حذف می‌شوند (rollback) و پیامی از خطا به همراه توضیحات به شما باز می‌گردد.

- **نام‌گذاری فایل‌ها**: فایل‌ها پس از دانلود با یک نام GUID تصادفی ذخیره می‌شوند تا از تداخل نام‌ها جلوگیری شود.

## نکات 📌

- اگر فایل به دلیل حجم یا پسوند نامعتبر نتواست دانلود شود، خطای مربوطه به صورت واضح به شما اعلام می‌شود.
- در صورت بروز خطا در هر مرحله‌ای از دانلود یا ذخیره‌سازی، عملیات به طور کامل متوقف می‌شود و تمام فایل‌های ذخیره‌شده حذف می‌شوند.

## همکاری 🤝

اگر پیشنهاد یا سوالی دارید، خوشحال می‌شویم که به ما اطلاع دهید و در توسعه این پروژه مشارکت کنید.

## نویسنده ✍️

- **ESH Team**  
