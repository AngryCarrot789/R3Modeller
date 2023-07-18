using System.Collections.Generic;

namespace R3Modeller.Core.AdvancedContextService {
    public interface IContextProvider {
        void GetContext(List<IContextEntry> list);
    }
}