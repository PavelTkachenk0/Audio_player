using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Files;

public abstract class BasePostFileValidator<TRequest> : BaseValidator<TRequest>
    where TRequest : BasePostFileRequest
{
    public BasePostFileValidator()
    {
        RuleFor(x => x)
            .Must(CheckExtensions)
            .WithErrorCode("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithErrorCode("one_of_the_files_is_too_large");
    }
    
    private bool CheckExtensions(TRequest req)
    {
        var extensions = Configuration.GetSection("FileStore:AudioExtensions").Get<string[]>();

        var files = HttpContextAccessor.HttpContext!.Request.Form.Files;

        var success = files.All(x => extensions!.Contains(Path.GetExtension(x.FileName)));
        return success;
    }

    private bool CheckFilesLenght(TRequest req)
    {
        var maxSize = Configuration.GetSection("FileStore:MaxSize").Get<int>();

        var files = HttpContextAccessor.HttpContext!.Request.Form.Files;

        var success = files.All(x => maxSize > x.Length);
        return success;
    }
}
