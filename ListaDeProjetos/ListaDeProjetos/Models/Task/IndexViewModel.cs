namespace ListaDeProjetos.Models.Task
{
    public class IndexViewModel
    {
        public DBModels.Project Project { get; set; }
        public List<DBModels.Task> Tasks { get; set; }
    }
}
