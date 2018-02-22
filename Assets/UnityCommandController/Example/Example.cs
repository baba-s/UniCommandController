using System;
using System.Collections.Generic;
using UnityCommandController;
using UnityEngine;

namespace UnityCommandController_Example
{
	public class Example : MonoBehaviour
	{
		private CommandController m_controller;

		private void Awake()
		{
			var commandTable = new Dictionary<string, Type>
			{
				{ "Log", typeof( LogCommand ) },
				{ "Create", typeof( CreateCommand ) },
				{ "SetPosition", typeof( SetPositionCommand ) },
				{ "Move", typeof( MoveCommand ) },
				{ "Jump", typeof( JumpCommand ) },
				{ "Wait", typeof( WaitCommand ) },
				{ "Click", typeof( ClickCommand ) },
			};

			m_controller = new CommandController( commandTable );

			var commands = new[]
			{
				"Log|ピカチュウ",
				"Log|ライチュウ",
				"Create|cube|0|1|2",
				"SetPosition|1|2|3",
				"Move|-1|0|1|1",
				"Jump|6",
				"Log|ここは無視されます",
				"Log|ここにジャンプします",
				"Wait|1",
				"Click",
			};

			GameObject cube = null;
			var startPos = Vector3.zero;
			var endPos = Vector3.zero;

			CreateCommand.OnCreate += go => cube = go;
			SetPositionCommand.OnSetPosition += pos => cube.transform.localPosition = pos;
			MoveCommand.OnMoveStart += pos =>
			{
				startPos = cube.transform.localPosition;
				endPos = pos;
			};
			MoveCommand.OnMove += amount =>
			{
				cube.transform.localPosition = Vector3.Lerp( startPos, endPos, amount );
			};
			MoveCommand.OnMoveEnd += () => cube.transform.localPosition = endPos;
			JumpCommand.OnJump += index => m_controller.JumpToIndex( index );

			m_controller.OnEnd += () => print( "終了" );
			m_controller.Start( commands );
		}

		private void Update()
		{
			m_controller.Update();
		}
	}

	public class LogCommand : CommandBase
	{
		private string m_message;

		public LogCommand( CommandArugments args )
		{
			m_message = args[ 1 ];
		}

		protected override void DoStart()
		{
			Debug.Log( m_message );
		}
	}

	public class CreateCommand : CommandBase
	{
		private string m_name;
		private Vector3 m_pos;

		public static event Action<GameObject> OnCreate = delegate { };

		public CreateCommand( CommandArugments args )
		{
			m_name = args[ 1 ];
			m_pos = new Vector3
			(
				args.ToFloat( 2 ),
				args.ToFloat( 3 ),
				args.ToFloat( 4 )
			);
		}

		protected override void DoStart()
		{
			var go = GameObject.CreatePrimitive( PrimitiveType.Cube );
			go.name = m_name;
			go.transform.localPosition = m_pos;
			OnCreate( go );
			Debug.Log( "オブジェクト作成完了" );
		}
	}

	public class SetPositionCommand : CommandBase
	{
		private Vector3 m_pos;

		public static event Action<Vector3> OnSetPosition = delegate { };

		public SetPositionCommand( CommandArugments args )
		{
			m_pos = new Vector3
			(
				args.ToFloat( 1 ),
				args.ToFloat( 2 ),
				args.ToFloat( 3 )
			);
		}

		protected override void DoStart()
		{
			OnSetPosition( m_pos );
			Debug.Log( "オブジェクト位置設定完了" );
		}
	}

	public class MoveCommand : CommandBase
	{
		private Vector3 m_pos;
		private float m_duration;
		private float m_elapsedTime;

		public override bool IsEnd { get { return m_duration <= m_elapsedTime; } }

		public static event Action<Vector3> OnMoveStart = delegate { };
		public static event Action<float> OnMove = delegate { };
		public static event Action OnMoveEnd = delegate { };

		public MoveCommand( CommandArugments args )
		{
			m_pos = new Vector3
			(
				args.ToFloat( 1 ),
				args.ToFloat( 2 ),
				args.ToFloat( 3 )
			);
			m_duration = args.ToFloat( 4 );
		}

		protected override void DoStart()
		{
			OnMoveStart( m_pos );
			Debug.Log( "オブジェクト移動開始" );
		}

		protected override void DoUpdate()
		{
			m_elapsedTime += Time.deltaTime;
			OnMove( m_elapsedTime / m_duration );
		}

		protected override void DoDispose()
		{
			OnMoveEnd();
			Debug.Log( "オブジェクト移動終了" );
		}
	}

	public class JumpCommand : CommandBase
	{
		private int m_index;

		public static event Action<int> OnJump = delegate { };

		public JumpCommand( CommandArugments args )
		{
			m_index = args.ToInt( 1 );
		}

		protected override void DoStart()
		{
			OnJump( m_index );
		}
	}

	public class WaitCommand : CommandBase
	{
		private float m_time;
		private float m_elapsedTime;

		public override bool IsEnd { get { return m_time <= m_elapsedTime; } }

		public WaitCommand( CommandArugments args )
		{
			m_time = args.ToFloat( 1 );
		}

		protected override void DoStart()
		{
			Debug.Log( "待機開始" );
		}

		protected override void DoUpdate()
		{
			m_elapsedTime += Time.deltaTime;
		}

		protected override void DoDispose()
		{
			Debug.Log( "待機終了" );
		}
	}

	public class ClickCommand : CommandBase
	{
		public override bool IsEnd { get { return Input.GetMouseButtonDown( 0 ); } }

		public ClickCommand( CommandArugments args ) { }

		protected override void DoStart()
		{
			Debug.Log( "入力待ち" );
		}

		protected override void DoDispose()
		{
			Debug.Log( "入力確認" );
		}
	}
}