namespace Notebook.WebClient.Test
{
    public class Test : IA, IB
    {
        int IA.Create()
        {
            return 2;
        }

        int IB.Create()
        {
            return 1;
        }
    }
}