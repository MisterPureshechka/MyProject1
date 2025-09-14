using System.Collections.Generic;

namespace Scripts.Messenger.ComeBackLogic
{
    public interface IComeBackStore
    {
        void Append(ComeBackRecord record);
        List<ComeBackRecord> PullAll(); 
    }
}