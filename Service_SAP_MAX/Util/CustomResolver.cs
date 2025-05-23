using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_SAP_MAX.Util
{
    public class CustomResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _ignorarCampos;

        public CustomResolver(IEnumerable<string> camposAExcluir)
        {
            _ignorarCampos = new HashSet<string>(camposAExcluir);
        }

        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization serialization)
        {
            var propiedad = base.CreateProperty(member, serialization);
            if (_ignorarCampos.Contains(propiedad.PropertyName))
            {
                propiedad.ShouldSerialize = _ => false;
            }
            return propiedad;
        }
    }
}
