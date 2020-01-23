namespace DatingApp.API.models
{
    public class Value
    {
        //whenever the var name is "Id", EntityFramework will recognize this as the primary key
        public int Id { get; set; }
        public string Name { get; set; }

    }
}