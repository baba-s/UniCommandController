using System;
using System.Collections.Generic;

namespace UnityCommandController
{
	/// <summary>
	/// コマンドのリストを管理するクラス
	/// </summary>
	public sealed class CommandList
	{
		private readonly List<string> m_list = new List<string>();  // コマンドのリスト

		/// <summary>
		/// 指定されたインデックスのコマンドを返します
		/// </summary>
		public string this[ int index ] { get { return m_list[ index ]; } }

		/// <summary>
		/// コマンドの数を返します
		/// </summary>
		public int Count { get { return m_list.Count; } }

		/// <summary>
		/// 指定された文字列のコレクションからコマンドのリストを作成します
		/// </summary>
		public CommandList( params string[] commands )
		{
			m_list = new List<string>( commands );
		}

		/// <summary>
		/// 指定された文字列のコレクションからコマンドのリストを作成します
		/// </summary>
		public CommandList( IEnumerable<string> commands )
		{
			m_list = new List<string>( commands );
		}

		/// <summary>
		/// 指定された文字列のコレクションからコマンドのリストを再作成します
		/// </summary>
		public void Set( CommandList commands )
		{
			m_list.Clear();
			m_list.AddRange( commands.m_list );
		}

		/// <summary>
		/// コマンドのリストを空にします
		/// </summary>
		public void Clear()
		{
			m_list.Clear();
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( Predicate<string> match )
		{
			return m_list.FindIndex( match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( int startIndex, Predicate<string> match )
		{
			return m_list.FindIndex( startIndex, match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindIndex( int startIndex, int count, Predicate<string> match )
		{
			return m_list.FindIndex( startIndex, count, match );
		}

		/// <summary>
		/// 指定された述語の条件を満たすコマンドのインデックスを返します
		/// </summary>
		public int FindLastIndex( Predicate<string> match )
		{
			return m_list.FindLastIndex( match );
		}

		/// <summary>
		/// 指定したコマンドのインデックスを返します
		/// </summary>
		public int FindIndexOfCommand( int startIndex, string commandName )
		{
			for ( var i = startIndex; i < m_list.Count; ++i )
			{
				var list = m_list[ i ];

				if ( list == null || list == string.Empty )
				{
					continue;
				}

				var command = list.Split( '|' );

				// 同じコマンドの場合
				if ( command[ 0 ] == commandName )
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 指定されたタグを含むコマンドのインデックスを返します
		/// </summary>
		public int FindIndexOfTag( int startIndex, string tag, int tagLine )
		{
			for ( var i = startIndex; i < m_list.Count; ++i )
			{
				var list = m_list[ i ];

				if ( list == null || list == string.Empty )
				{
					continue;
				}

				var command = list.Split( '|' );

				if ( command.Length == tagLine )
				{
					continue;
				}

				// タグが一致した場合インデックスを返す
				if ( command[ tagLine ] == tag )
				{
					return i;
				}
			}
			return -1;
		}
	}
}
