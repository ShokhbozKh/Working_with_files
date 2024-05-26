using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Working_with_files.Controllers;

[Route("api/Files")]
[ApiController]
public class FilesController : ControllerBase
{
    private FileExtensionContentTypeProvider _provider;

    public FilesController(FileExtensionContentTypeProvider provider)
    {
        _provider = provider;
    }

    [HttpGet("GetFiles/{id}")]
    public ActionResult GetFiles(int id)
    {
        var path = "AllFiles\\germany.pdf"; // file location

        if(!System.IO.File.Exists(path)) // file bor yoki yuqligi
        {
            return NotFound("File not found");
        }

        if(!_provider.TryGetContentType(path, out var contentType)) // file type ni aniqlash
        {
            contentType = "application/octet-stream"; // topa olamasa 
        }

        var bytes = System.IO.File.ReadAllBytes(path); // file ni path bo'yicha uqish byte larda
        var result = File(bytes,contentType, Path.GetFileName(path)); // file ni qaytarish ui ga
        //Path.GetFileName(path) ->> bu pathdagi file ni yuqlab olish uchun  qaytaradi file ko'rinishida

        return result;
    }

    [HttpPost]
    public async Task< ActionResult> CreateFile(IFormFile file)
    {
        if(file == null) // file bushligini aniqlash
        {
            return BadRequest();
        }

        //if(file.Length !> 10240)
        //{
        //    return BadRequest("File size not limited");
        //}

        if(file.ContentType != "application/pdf") // file type ni aniqlash
        {
            return BadRequest($" file type not correct");
        }

        

        var path = Path.Combine(Directory.GetCurrentDirectory() + "\\AllFiles",
                    $"uploaded_file_{GetHashCode()}.pdf") ;  //  file ni  birlashtirish
                                     //Guid.NewGuid
        using (var stream = new FileStream(path, FileMode.Create)) // fileni saqlash
        {
            await file.CopyToAsync(stream);
        }

        return Ok("Your file has been saved");

    }
}
