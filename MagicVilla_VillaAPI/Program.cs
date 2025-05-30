
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);


var app = builder.Build();
app.UseSwagger();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
	}
	);
}else
{
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
		options.RoutePrefix = ""; // when change the env-var into production you need to comment this line at least for testing 
	}
	);
}

app.UseExceptionHandler("/ErrorHandling/ProcessError");
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