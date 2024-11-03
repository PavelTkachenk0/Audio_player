namespace Audio_player.Validators;

public abstract class BaseAudioFileValidator<TRequest> : BaseValidator<TRequest>
    where TRequest : class
{
    protected virtual bool CheckExtensions(IFormFile file)
    {
        var extensions = Configuration.GetSection("AudioStore:Extensions").Get<string[]>();

        var success = extensions!.Contains(Path.GetExtension(file.FileName).ToLower());

        return success;
    }

    protected virtual bool CheckFilesLenght(IFormFile file)
    {
        var maxSize = Configuration.GetSection("AudioStore:MaxSize").Get<int>();

        var success = maxSize > file.Length;

        return success;
    }
}
