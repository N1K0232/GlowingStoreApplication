namespace GlowingStoreApplication.Models;

public class FormFileContent
{
    public FormFileContent(IFormFile file)
    {
        File = file;
    }

    public IFormFile File { get; }

    public static async ValueTask<FormFileContent> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        if (form is null)
        {
            return null;
        }

        var file = form.Files[0];
        if (file is null)
        {
            return null;
        }

        return new FormFileContent(file);
    }
}