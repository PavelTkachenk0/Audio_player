using Audio_player.Models.Requests;
using FluentValidation;

namespace Audio_player.Validators.Files;

public abstract class BasePostImageValidator<TRequest> : BaseValidator<TRequest>
    where TRequest : BasePostImageRequest
{
    public BasePostImageValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtensions)
            .WithMessage("incorrect_file_extension")
            .Must(CheckFilesLenght)
            .WithMessage("one_of_the_files_is_too_large");
    }
    
    private bool CheckExtensions(TRequest req)
    {
        var extensions = Configuration.GetSection("ImageStore:Extensions").Get<string[]>();

        var files = HttpContextAccessor.HttpContext!.Request.Form.Files;

        var success = files.All(x => extensions!.Contains(Path.GetExtension(x.FileName).ToLower()));
        return success;
    }

    private bool CheckFilesLenght(TRequest req)
    {
        var maxSize = Configuration.GetSection("ImageStore:MaxSize").Get<int>();

        var files = HttpContextAccessor.HttpContext!.Request.Form.Files;

        var success = files.All(x => maxSize > x.Length);
        return success;
    }
}
