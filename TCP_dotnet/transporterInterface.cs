using System.Threading.Tasks;

namespace TransporterSetups{

    public interface ImessageTransporter {
        public Task sendMessage(string Message);
    }
}