namespace MVCParamBuilder.Models
{
    public class Test
    {
        public InnerTest InnerTest { get; set; }
    }

    public class InnerTest
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public MoreInnerTest MoreInnerTest { get; set; }
    }

    public class MoreInnerTest
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}