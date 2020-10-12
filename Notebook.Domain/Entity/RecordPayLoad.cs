using System.Collections.Generic;

namespace Notebook.Domain.Entity
{
    public class RecordPayLoad 
    {
        private bool? _send;
        private IEnumerable<int> _columnsId;

        public IEnumerable<int> ColumnsId
        {
            get => _columnsId;
            set => _columnsId = value ?? new List<int>();
        }

        public bool? Send
        {
            get => _send;
            set => _send = value?? false; //allow to aviod problem with null during deserial если доб поле в предыдущ версиях джейсона его нет при дес он автоматом поставит налл но чтобы сделать обратную совместимость с джейсоном нужна будет такая фишка на любое новое поле не рассчитанное на налл. За счет такой вещи мы невилруем прлблему обратной совместимость джейсона.мы вытащим налл но на фрон выдам нормальное знач кото потом сохзраниться в бд
        } //для обратной совместимостьи удем пихать знач по умолчанию
    }
}