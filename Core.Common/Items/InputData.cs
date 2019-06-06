namespace Core.Common.Items
{
    public class InputData
    {
        public InputData
        (
            string[] columns, 
            string[] rows        
        )
        {
            Columns = columns;
            Rows = rows;
        }
        public string[] Columns { get; }
        public string[] Rows   { get; }

}
}
