
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

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ApplyMigration(); // No Need For Update DB after Migration 
app.Run();


void ApplyMigration()
{
	using(var scope = app.Services.CreateScope())
	{
		var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		if(_db.Database.GetPendingMigrations().Count() > 0)
			_db.Database.Migrate(); 
	}
}