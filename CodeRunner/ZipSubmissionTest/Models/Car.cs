namespace ZipSubmissionTest.Models
{
    public class Car
    {
        public string Model { get; set; }

        public int Year { get; set; }

        public override string ToString()
        {
            return $"{this.Model} => {this.Year}";
        }
    }
}
