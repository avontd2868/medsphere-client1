using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity;

namespace ClinSchd.Infrastructure
{
	public class Factory<T>
	{

		#region IFactory<T> Members

		public T Create ()
		{
			return _kernel.Resolve<T> ();
		}

		#endregion

		#region public methods
		public Factory (IUnityContainer kernel)
		{
			_kernel = kernel;
		}
		#endregion

		#region private fields
		readonly IUnityContainer _kernel;
		#endregion
	}

	//public class Factory<T, TARG1>
	//{
	//    #region IFactory<T,TARG1> Members

	//    public T Create (TARG1 one)
	//    {
	//        return _kernel.Resolve<T>(new ParameterOverrides());.Get<T> (new Parameters.ConstructorArgument (_arg_one_name, one));
	//    }

	//    #endregion

	//    #region public methods

	//    public Factory (string arg_one_name, IKernel kernel)
	//    {
	//        _kernel = kernel;
	//        _arg_one_name = arg_one_name;
	//    }

	//    #endregion

	//    #region private fields

	//    readonly IUnityContainer _kernel;
	//    readonly string _arg_one_name;

	//    #endregion
	//}

	//public class Factory<T, TARG1, TARG2>
	//{
	//    #region IFactory<T,TARG1> Members

	//    public T Create (TARG1 one, TARG2 two)
	//    {
	//        return _kernel.Get<T> (new Parameters.ConstructorArgument (_arg_one_name, one),
	//            new Parameters.ConstructorArgument (_arg_two_name, two));
	//    }

	//    #endregion

	//    #region public methods

	//    public Factory (string arg_one_name, string arg_two_name, IKernel kernel)
	//    {
	//        _kernel = kernel;
	//        _arg_one_name = arg_one_name;
	//        _arg_two_name = arg_two_name;
	//    }

	//    #endregion

	//    #region private fields

	//    readonly IUnityContainer _kernel;
	//    readonly string _arg_one_name, _arg_two_name;

	//    #endregion
	//}

	//public class Factory<T, TARG1, TARG2, TARG3>
	//{
	//    #region IFactory<T,TARG1,TARG2,TARG3> Members

	//    public T Create (TARG1 one, TARG2 two, TARG3 three)
	//    {

	//        return _kernel.Get<T> (new Parameters.ConstructorArgument (_arg_one_name, one),
	//            new Parameters.ConstructorArgument (_arg_two_name, two),
	//            new Parameters.ConstructorArgument (_arg_three_name, three));
	//    }

	//    #endregion

	//    #region public methods

	//    public Factory (string arg_one_name,
	//        string arg_two_name,
	//        string arg_three_name,
	//        IKernel kernel)
	//    {
	//        _kernel = kernel;
	//        _arg_one_name = arg_one_name;
	//        _arg_two_name = arg_two_name;
	//        _arg_three_name = arg_three_name;
	//    }

	//    #endregion

	//    #region private fields

	//    readonly IUnityContainer _kernel;
	//    readonly string _arg_one_name, _arg_two_name, _arg_three_name;

	//    #endregion
	//}

	//public class Factory<T, TARG1, TARG2, TARG3, TARG4>
	//{
	//    #region IFactory<T,TARG1,TARG2,TARG3> Members

	//    public T Create (TARG1 one, TARG2 two, TARG3 three, TARG4 four)
	//    {

	//        return _kernel.Get<T> (new Parameters.ConstructorArgument (_arg_one_name, one),
	//            new Parameters.ConstructorArgument (_arg_two_name, two),
	//            new Parameters.ConstructorArgument (_arg_three_name, three),
	//            new Parameters.ConstructorArgument (_arg_four_name, four));
	//    }

	//    #endregion

	//    #region public methods

	//    public Factory (string arg_one_name,
	//        string arg_two_name,
	//        string arg_three_name,
	//        string arg_four_name,
	//        IKernel kernel)
	//    {
	//        _kernel = kernel;
	//        _arg_one_name = arg_one_name;
	//        _arg_two_name = arg_two_name;
	//        _arg_three_name = arg_three_name;
	//        _arg_four_name = arg_four_name;
	//    }

	//    #endregion

	//    #region private fields

	//    readonly IUnityContainer _kernel;
	//    readonly string _arg_one_name, _arg_two_name, _arg_three_name, _arg_four_name;

	//    #endregion
	//}
}
