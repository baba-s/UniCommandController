using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommandController
{
	/// <summary>
	/// コマンドを制御するクラス
	/// </summary>
	public sealed class CommandController : IDisposable
	{
		private readonly CommandList m_commandList = new CommandList();	// コマンドのリスト

		private int			m_currentIndex;		// 現在のインデックス
		private int			m_jumpIndex;		// ジャンプ先のインデックス
		private ICommand	m_currentCommand;	// 現在のコマンド
		private bool		m_isEnd;			// 終了したかどうか

		private string m_separator = "|";	// コマンドの引数の区切り文字列

		private readonly Dictionary<string, Type> m_commandTable = new Dictionary<string, Type>();

		/// <summary>
		/// 現在のコマンドのインデックスを返します
		/// </summary>
		public int CurrentIndex { get { return m_currentIndex; } }

		/// <summary>
		/// コマンドの数を返します
		/// </summary>
		public int Count { get { return m_commandList.Count; } }

		/// <summary>
		/// コマンドの引数の区切り文字列を取得または設定します
		/// </summary>
		public string Separator { set { m_separator = value; } }

		/// <summary>
		/// 終了した場合に呼び出されます
		/// </summary>
		public event Action OnEnd = delegate { };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CommandController( Dictionary<string, Type> commandTable )
		{
			m_commandTable = commandTable;
		}

		/// <summary>
		/// 指定されたコマンドのリストでイベントを開始します
		/// </summary>
		public void Start( params string[] commands )
		{
			Start( new CommandList( commands ) );
		}

		/// <summary>
		/// 指定されたコマンドのリストでイベントを開始します
		/// </summary>
		public void Start( CommandList commandList )
		{
			m_commandList.Set( commandList );

			Restart();
		}

		/// <summary>
		/// コマンドを更新します
		/// </summary>
		public void Update()
		{
			// すべてのコマンドが終了している場合は処理を行いません
			if ( m_commandList.Count <= m_currentIndex )
			{
				return;
			}

			// 現在のコマンドを更新します
			m_currentCommand.Update();

			// 現在のコマンドが終了した場合は
			// 次のコマンドに進みます
			if ( m_isEnd || m_currentCommand.IsEnd )
			{
				m_isEnd = false;

				// ジャンプ先のインデックスが指定された場合は
				// ジャンプ先のコマンドに進みます
				Change( m_jumpIndex != -1 ? m_jumpIndex : m_currentIndex + 1 );

				// ジャンプ先のインデックスへ移動が完了した場合はジャンプ先のインデックスを無効な状態にします
				if ( m_currentIndex == m_jumpIndex )
				{
					m_jumpIndex = -1;
				}
			}

			// すべてのコマンドが終了した場合はイベントを呼び出します
			if ( m_commandList.Count <= m_currentIndex )
			{
				OnEnd();
			}
		}

		/// <summary>
		/// 更新するコマンドを変更します
		/// </summary>
		private void Change( int index )
		{
			// 移動先のインデックスが現在のインデックスと同じ場合は何もしません
			if ( m_currentIndex == index )
			{
				return;
			}

			// 現在のコマンドを終了します
			m_currentCommand.Dispose();

			// すべてのコマンドが終了した場合は次のコマンドに進まず終了します
			m_currentIndex = index;
			if ( m_commandList.Count <= m_currentIndex )
			{
				return;
			}

			// 次のコマンドを作成して開始します
			m_currentCommand = CreateCommand( m_commandList[ m_currentIndex ] );
			m_currentCommand.Start();

			// 次のコマンドが終了した場合はさらにその次のコマンドに進みます
			if ( !m_currentCommand.IsEnd )
			{
				return;
			}

			Change( m_jumpIndex != -1 ? m_jumpIndex : m_currentIndex + 1 );
		}

		/// <summary>
		/// コマンドを指定されたインデックスまでジャンプします
		/// </summary>
		public void JumpToIndex( int index )
		{
			m_jumpIndex = index;
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( Predicate<string> match )
		{
			return m_commandList.FindIndex( match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( int startIndex, Predicate<string> match )
		{
			return m_commandList.FindIndex( startIndex, match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( int startIndex, int count, Predicate<string> match )
		{
			return m_commandList.FindIndex( startIndex, count, match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindLastIndex( Predicate<string> match )
		{
			return m_commandList.FindLastIndex( match );
		}

		/// <summary>
		/// コマンドのリストを初期化します
		/// </summary>
		public void Clear()
		{
			m_commandList.Clear();
		}

		/// <summary>
		/// イベントを最初から開始します
		/// </summary>
		public void Restart()
		{
			m_currentIndex = 0;
			m_jumpIndex = -1;
			m_currentCommand = CreateCommand( m_commandList[ m_currentIndex ] );
			m_currentCommand.Start();
		}

		/// <summary>
		/// イベントを終了します
		/// </summary>
		public void End()
		{
			m_jumpIndex = m_commandList.Count;
			m_isEnd = true;
		}

		/// <summary>
		/// 破棄します
		/// </summary>
		public void Dispose()
		{
			OnEnd = delegate { };
		}

		/// <summary>
		/// 指定された文字列からコマンドを生成します
		/// コマンドのパラメータは「|」で区切る必要があります
		/// 例)
		/// Text|田中太郎|こんにちは
		/// ↓
		/// new TextCommand(new ParamList( new [] { "田中太郎", "こんにちは" } ))
		/// </summary>
		private ICommand CreateCommand( string command )
		{
			var scriptList = command.Split( new [] { m_separator }, StringSplitOptions.None ).ToList();
			var type = m_commandTable[ scriptList[ 0 ] ];

			var script = ( ICommand )Activator.CreateInstance( type, new object[] { new CommandArugments( scriptList ) } );

			return script;
		}
	}
}
