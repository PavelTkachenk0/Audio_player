using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Files;

public abstract class BaseFileValidator<TRequest> : BaseValidator<TRequest>
    where TRequest : class
{
    protected virtual bool CheckExtensions(IFormFile file)
    {
        var extensions = Configuration.GetSection("ImageStore:Extensions").Get<string[]>();

        var success = extensions!.Contains(Path.GetExtension(file.FileName).ToLower());

        return success;
    }

    protected virtual bool CheckFilesLenght(IFormFile file)
    {
        var maxSize = Configuration.GetSection("ImageStore:MaxSize").Get<int>();

        var success = maxSize > file.Length;

        return success;
    }
}
