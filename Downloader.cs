namespace ESH.FileDownloader;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// تایپ لیستی که برای این کتابخانه ارسال میشود
/// </summary>
public enum FileType
{
    Images,
    Pdf,
    Video,
    Music
}

/// <summary>
/// خروجی به ازای هر فایل
/// </summary>
public class FileResult : object
{
    public FileResult() : base()
    {
    }
    
    /// <summary>
    /// نام اصلی فایل
    /// </summary>
    public string OriginalName { get; set; }
    
    /// <summary>
    /// نام رندوم برای ذخیره در سرور ما
    /// </summary>
    public string GuidName { get; set; }
    
    /// <summary>
    /// خطای هنگام دانلود و ثبت
    /// </summary>
    public string Error { get; set; }

    public bool HasError
    {
        get
        {
            bool resutl = !string.IsNullOrEmpty(Error);
            return resutl;
        }
    }
}

public class Downloader : object
{
    
    /// <summary>
    /// جهت مدیریت دقیق پسوند و حجم فایل ها
    /// </summary>
    private static readonly Dictionary<FileType, (List<string> Extensions, long MaxSize)> FileRules = new()
    {
        { FileType.Images, ([".jpg", ".jpeg", ".png", ".gif"], 1 * 1024 * 1024) },  // 1 MB
        { FileType.Pdf, ([".pdf"], 20 * 1024 * 1024) },                             // 20 MB
        { FileType.Video, ([".mp4", ".avi", ".mkv", ".mov"], 3 * 1024 * 1024) },    // 3 MB
        { FileType.Music, ([".mp3", ".wav", ".flac"], 3 * 1024 * 1024) }            // 3 MB
    };

      public static async Task<List<FileResult>> DownloadAndSaveFilesAsync(
          List<string> fileUrls,
          FileType fileType,
          string saveDirectory)
    {
        List<FileResult> results = new();
        List<string> savedFiles = new();

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        using HttpClient client = new();
        
        var (validExtensions, maxSize) = FileRules[fileType];

        foreach (var url in fileUrls)
        {
            var result = new FileResult();
            try
            {
                // Get original file name
                var originalName = Path.GetFileName(new Uri(url).LocalPath);
                result.OriginalName = originalName;

                // Check extension
                var extension = Path.GetExtension(originalName).ToLower();
                if (!validExtensions.Contains(extension))
                {
                    result.Error = "Invalid file extension for the selected file type.";
                    throw new Exception(result.Error);
                }

                // Download file
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Failed to download file. HTTP Status: {response.StatusCode}";
                    throw new Exception(result.Error);
                }

                var fileData = await response.Content.ReadAsByteArrayAsync();

                // Check file size
                if (fileData.Length > maxSize)
                {
                    result.Error = $"File size exceeds the limit of {maxSize / (1024 * 1024)} MB.";
                    throw new Exception(result.Error);
                }

                // Save file with GUID name
                var guidName = $"{Guid.NewGuid()}{extension}";
                var savePath = Path.Combine(saveDirectory, guidName);
                await File.WriteAllBytesAsync(savePath, fileData);

                result.GuidName = guidName;
                savedFiles.Add(savePath); // Track saved file
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;

                // Rollback: Delete all successfully saved files
                foreach (var savedFile in savedFiles)
                {
                    try
                    {
                        if (File.Exists(savedFile))
                        {
                            File.Delete(savedFile);
                        }
                    }
                    catch
                    {
                        // Log error silently (optional)
                    }
                }

                results.Add(result);
                return results; // Exit early on error
            }

            results.Add(result);
        }

        return results;
    }
    
      
    public static void DeleteFiles(List<string?> fileNames, string folderPath)
    {
        foreach (var fileName in fileNames)
        {
            try
            {
                var filePath = Path.Combine(folderPath, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // ignored
            }
        }
    }

}
