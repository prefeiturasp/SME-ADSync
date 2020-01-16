namespace SME.SyncADxCoreSSO.Comandos
{
    public interface IComando
    {
        void Executar(params string[] args);
    }
}