namespace MagicVilla_VillaAPI.Models;

// we are no longer need this model after identity
public class LocalUser

{
	public int Id { get; set; }
	public string UserName { get; set; }
	public string Name { get; set; }
	public string Password { get; set; }
	public string Role { get; set; }

}
