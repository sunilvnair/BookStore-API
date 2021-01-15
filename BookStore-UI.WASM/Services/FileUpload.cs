using BlazorInputFile;
using BookStore_UI.WASM.Contracts;
//using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Services
{
    //public class FileUpload : iFileUpload
    //{
    //    private readonly IWebHostEnvironment _env;
    //    public FileUpload(IWebHostEnvironment env)
    //    {
    //        _env = env;
    //    }
    //    public  async Task <bool> UploadFile(IFileListEntry file, string picName)
    //    {
    //        try
    //        {
    //            var ms = new MemoryStream();
    //            await file.Data.CopyToAsync(ms);
    //            var path = $"{_env.WebRootPath}\\uploads\\{picName}";

    //            using (FileStream fs = new FileStream(path,FileMode.Create))
    //            {
    //                ms.WriteTo(fs);
    //            }
    //            return true;
    //        }

    //        catch (Exception)
    //        {

    //            return false;
    //        }
    //    }

    //    public void UploadFile(IFileListEntry file, MemoryStream msFile, string picName)
    //    {
    //        try
    //        {
    //            var path = $"{_env.WebRootPath}\\uploads\\{picName}";

    //            using (FileStream fs = new FileStream(path, FileMode.Create))
    //            {
    //                msFile.WriteTo(fs);
    //            }

    //        }
    //        catch (Exception ex)
    //        {

    //            throw;
    //        }
    //    }
    //    public void RemoveFile(string picName)
    //    {
    //        var path = $"{_env.WebRootPath}\\uploads\\{picName}";
    //        if (File.Exists(path))
    //        {
    //            File.Delete(path);
    //        }
    //    }
    //}
}
