using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class ParameterList : IReadOnlyList<UIParameter>
    {
        [JsonProperty]
        private IReadOnlyList<UIParameter> _parameters;

        public UIParameter this[int index] => _parameters[index];

        public int Count => _parameters.Count;

        IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();

        public IEnumerator<UIParameter> GetEnumerator() => _parameters.GetEnumerator();

        // Not using a hashmap because we need this to be ordered and 99% of the time we will only
        // iterate over the list to draw the UI so a list makes way more sense. Also these O(n)
        // methods will rarely be used. ( if ever )
        public Parameter? Get(string fullName)
        {
            foreach (UIParameter uiParameter in _parameters)
            {
                if (uiParameter.FullName == fullName)
                    return uiParameter.Parameter;
            }
            return null;
        }

        public ParameterList(params UIParameter[] parameters)
        {
            _parameters = new List<UIParameter>(parameters);
        }

        [JsonConstructor]
        public ParameterList(List<UIParameter> parameters)
        {
            _parameters = parameters;
        }

        public ParameterList(IReadOnlyList<UIParameter> parameters)
        {
            _parameters = parameters;
        }
    }
}
