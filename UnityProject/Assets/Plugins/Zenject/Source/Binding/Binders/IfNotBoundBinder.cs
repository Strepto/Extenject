using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    [NoReflectionBaking]
    public class IfNotBoundBinder
    {
        public IfNotBoundBinder(BindInfo bindInfo)
        {
            BindInfo = bindInfo;
        }

        // Do not use this
        public BindInfo BindInfo
        {
            get;
            private set;
        }

        public void IfNotBound()
        {
            BindInfo.OnlyBindIfNotBound = true;
        }

        public void IfNotBound<T>() where T : class
        {
            AssertCanBeBoundToObject(typeof(T));

            BindInfo.OnlyBindIfNotBoundTypes = new List<Type>() { typeof(T) };
        }

        public void IfNotBound(params Type[] types)
        {
            AssertCanBeBoundToObject(types);

            BindInfo.OnlyBindIfNotBoundTypes = types.ToList();
        }

        private void AssertCanBeBoundToObject(params Type[] types)
        {
            if (!types.All(x => BindInfo.ContractTypes.Contains(x)))
            {
                throw Assert.CreateException(
                                    $"Expected ContractTypes '{string.Join(", ", BindInfo.ContractTypes)}' to contain types '{string.Join<Type>(", ", types)}' when {BindInfo.ContextInfo}");
            }
        }
    }
}

