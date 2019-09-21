using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestIfNotBound : ZenjectUnitTestFixture
    {
        interface IFoo
        {
        }

        interface IBar
        {
        }

        public class Foo1 : IFoo
        {
        }

        public class Foo2 : IFoo
        {
        }

        public class Foo3 : IFoo, IInitializable
        {
            public bool IsInitialized = false;
            public void Initialize()
            {
                IsInitialized = true;
            }
        }

        public class Bar1 : IBar, IInitializable
        {
            public bool IsInitialized = false;
            public void Initialize()
            {
                IsInitialized = true;
            }
        }

        [Test]
        public void Test1()
        {
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.Bind<IFoo>().To<Foo2>().AsSingle();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
        }

        [Test]
        public void Test2()
        {
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().IfNotBound();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 1);
            Assert.IsType<Foo1>(Container.Resolve<IFoo>());
        }

        [Test]
        public void Test3()
        {
            // This test asserts "Unwanted" behavior, and should be removed if this behavior is changed.
            Container.BindInterfacesTo<Bar1>().AsSingle();
            Container.BindInterfacesTo<Foo3>().AsSingle().IfNotBound();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 1);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 1);

            // The IInitializable interface is already be bound by Bar1, 
            // and therefore the IfNotBound triggers and does not bind IInitializable to Foo3
            Assert.IsEqual(Container.ResolveAll<IInitializable>().Count, 1);
        }

        [Test]
        public void Test3_Conditional()
        {
            // This test asserts the conditional behavior of Test 3.
            Container.BindInterfacesTo<Bar1>().AsSingle();
            Container.BindInterfacesTo<Foo3>().AsSingle().IfNotBound<IFoo>();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 1);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 1);

            // The IInitializable interface binding is bound twice!
            Assert.IsEqual(Container.ResolveAll<IInitializable>().Count, 2);
        }

        [Test]
        public void Test4()
        {
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.BindInterfacesTo<Foo3>().AsSingle().IfNotBound<IFoo>();

            Assert.IsEqual(Container.ResolveAll<IInitializable>().Count, 0);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 1);
            Assert.IsType<Foo1>(Container.Resolve<IFoo>());
        }

        [Test]
        public void Test5()
        {
            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.BindInterfacesTo<Foo3>().AsSingle().IfNotBound(typeof(IFoo));

            Assert.IsEqual(Container.ResolveAll<IInitializable>().Count, 0);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 1);
            Assert.IsType<Foo1>(Container.Resolve<IFoo>());
        }

        [Test]
        public void Test6()
        {
            // Can only "IfNotBound<T>" interfaces that could have been bound. (Consider if this should have an override possibility)
            Assert.Throws<ZenjectException>( () => Container.BindInterfacesTo<Foo3>().AsSingle().IfNotBound<IBar>());
        }
    }
}
