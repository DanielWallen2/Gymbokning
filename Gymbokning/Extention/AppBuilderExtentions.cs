using Gymbokning.Data;

namespace Gymbokning.Extention
{
    public static class AppBuilderExtentions
    {
        public static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

                //db.Database.EnsureDeleted();                      // ?
                //db.Database.Migrate();                            // ?
                //dotnet user-secrets set "AdminPW" "BytMig123!"    // ?

                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var adminPSW = configuration["AdminPW"];

                try
                {
                    await SeedData.InitAsync(context, serviceProvider, adminPSW);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return app;
        }
    }
}
