using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using FeatureDemandPlanning.Schematron.Formatters;

namespace FeatureDemandPlanning.Schematron
{
	/// <summary>
	/// Base class for Schematron evaluation contexts.
	/// </summary>
	/// <remarks>
	/// The schematron elements don't provide evaluation code. They just
	/// represent the elements in the <link ref="schematron" />.
	/// <para>
	/// For evaluation purposes, this class provides the iteration and execution
	/// process, accumulates results, moves the cursor, etc. Here is where the 
	/// actual evaluation takes place. This way we isolate the schema design
	/// from the different execution models we can use, for example, 
	/// <see cref="SyncEvaluationContext"/> (we may add asynchonous execution later).
	/// </para>
	/// <para>
	/// The validator classes provided use instances of this strategy object to 
	/// validate documents provided by the user.
	/// </para>
	/// </remarks>
	/// <author ref="dcazzulino" />
	/// <progress amount="100" />
	public abstract class EvaluationContextBase
	{
		/// <summary>
		/// Keeps a list of nodes already matched.
		/// </summary>
		/// <remarks>
		/// When the <seealso cref="Source"/> property is set, the appropriate
		/// strategy for matching nodes is initialized, depending on the specific
		/// implementation of the <see cref="XPathNavigator"/> in use.
		/// </remarks>
		protected IMatchedNodes Matched;
		
		/// <summary>Creates the evaluation context</summary>
		public EvaluationContextBase()
		{
		}

		#region Properties
		IFormatter _formatter = Config.DefaultFormatter;

		/// <summary>Gets or sets the class to use to format messages.</summary>
		/// <remarks>
		/// This object is initialized to the <see cref="Config.DefaultFormatter"/> instance.
		/// Usually, it will be changed based on parameters passed to the validator class,
		///	or exposed directly by it.
		/// </remarks>
		public IFormatter Formatter
		{
			get { return _formatter; }
			set { _formatter = value; }
		}

		StringBuilder _messages = new StringBuilder();

		/// <summary>Gets or sets the messages generated by the validation process.</summary>
		/// <remarks>
		/// Specific implementations of this class read/write this property 
		/// while they accumulate validation messages.
		/// </remarks>
		public StringBuilder Messages
		{
			get { return _messages; }
			set { _messages = value; }
		}

		bool _haserrors = false;

		/// <summary>Indicates if errors were found during the current evaluation.</summary>
		public bool HasErrors
		{
			get { return _haserrors; }
			set { _haserrors = value; }
		}

		string _phase = String.Empty;

		/// <summary>Gets or sets the specific validation phase to run.</summary>
		/// <remarks>
		/// Schematron supports the concept of phases, where different sets of 
		/// patterns can be executed at different times. This phase is initialized
		/// to <see cref="String.Empty"/>, which will mean all patterns are run.
		/// </remarks>
		public string Phase
		{
			get { return _phase; }
			set { _phase = value; }
		}

		Schema _schema;

		/// <summary>Gets or sets the schema to use for the validation.</summary>
		public Schema Schema
		{
			get { return _schema; }
			set { _schema = value; }
		}

		XPathNavigator _source;

		/// <summary>Gets or sets the document to validate.</summary>
		/// <remarks>
		/// When this property is set, the appropriate <see cref="IMatchedNodes"/>
		/// strategy is picked, to perform optimum for various navigator implementations.
		/// </remarks>
		public XPathNavigator Source
		{
			get { return _source; }
			set 
			{ 
				_source = value; 
				if (value is IHasXmlNode)
				{
					Matched = new DomMatchedNodes();
				}
				else if (value is IXmlLineInfo)
				{
					Matched = new XPathMatchedNodes();
				}
				else
				{
					Matched = new GenericMatchedNodes();
				}
			}
		}
		#endregion

		/// <summary>
		/// Starts the evaluation process.
		/// </summary>
		/// <remarks>
		/// When the process is finished, the results are placed 
		/// in the <seealso cref="Messages"/> property.
		/// </remarks>
		public abstract void Start();

		/// <summary>
		/// Resets the state of the current context.
		/// </summary>
		/// <remarks>
		/// By default, it clears the <see cref="Messages"/> and sets <see cref="HasErrors"/> to false.
		/// </remarks>
		protected void Reset()
		{
			_messages = new StringBuilder();

		}
	}
}