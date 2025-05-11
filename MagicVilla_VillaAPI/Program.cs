
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
	app.UseSwaggerUI(options => 
	{
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
	}
	);
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
