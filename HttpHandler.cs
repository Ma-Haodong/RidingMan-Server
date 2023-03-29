using System.Text;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

public static class HttpHandler
{
    public static void AddHttpHandler(this WebApplication app)
    {
        app.MapPost("/register", async httpContext =>
        {
            using (RidingManContext dbContext = new RidingManContext())
            {
                string name = httpContext.Request.Form["name"];
                if (dbContext.Users.FirstOrDefault(user => user.Name == name) == null)
                {
                    string id = Guid.NewGuid().ToString();
                    dbContext.Users.Add(new User { Id = id, Name = name, Password = httpContext.Request.Form["password"] });
                    await dbContext.SaveChangesAsync();
                    foreach (var item in httpContext.Request.Form.Files)
                    {
                        string iconName = $"{id}.png";
                        Stream stream = item.OpenReadStream();
                        byte[] data = new byte[stream.Length];
                        await stream.ReadAsync(data, 0, (int)stream.Length);
                        await File.WriteAllBytesAsync($"{Directory.GetCurrentDirectory()}/icons/{iconName}", data);
                    }
                    await httpContext.Response.WriteAsJsonAsync(new { success = true });
                }
                else
                {
                    await httpContext.Response.WriteAsJsonAsync(new { success = false, message = "The name already exist!" });
                }
            }
        });

        app.MapPost("/login", async httpContext =>
        {
            using (RidingManContext dbContext = new RidingManContext())
            {
                string name = httpContext.Request.Form["name"];
                User user = dbContext.Users.FirstOrDefault(user => user.Name == name);
                if (user == null)
                {
                    await httpContext.Response.WriteAsJsonAsync(new { success = false, message = "The name does not exist!" });
                }
                else
                {
                    if (user.Password == httpContext.Request.Form["password"])
                    {
                        await httpContext.Response.WriteAsJsonAsync(new { success = true, id = user.Id, name = user.Name });
                    }
                    else
                    {
                        await httpContext.Response.WriteAsJsonAsync(new { success = false, message = "The password is incorrect!" });
                    }
                }
            }
        });

        app.MapGet("/getIcon", async httpContext =>
        {
            string path = $"{Directory.GetCurrentDirectory()}/icons/{httpContext.Request.Form["id"].ToString()}.png";
            if (File.Exists(path))
            {
                byte[] data = await File.ReadAllBytesAsync(path);
                await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
            }
            else
            {
                await httpContext.Response.Body.WriteAsync(new byte[] { }, 0, 0);
            }
        });
    }
}