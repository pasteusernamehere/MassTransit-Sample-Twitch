using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Sample.Api;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample.Api v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();