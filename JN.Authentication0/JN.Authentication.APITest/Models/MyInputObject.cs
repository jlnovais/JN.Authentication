namespace AuthenticationDemo.Models
{
    public class MyInputObject
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{{\"Name\":\"{Name}\", \"Address\": \"{Address}\" }}";
        }
    }
}