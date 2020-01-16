namespace SME.ADSync.Core.Interfaces
{
    public interface IRepositorioAD : IRepositorio
    {
        void AtualizarSenha(string login, string senha);
        void DesativarUsuario(string login);
    }
}
