using System;
using System.Collections.Generic;

namespace UniCommandController
{
	/// <summary>
	/// コマンドのコンストラクタに渡される引数を管理するクラス
	/// </summary>
	public sealed class CommandArguments
	{
		private readonly List<string> m_list; // 引数を管理するリスト

		/// <summary>
		/// 指定されたインデックスの引数を返します
		/// </summary>
		public string this[ int index ] => m_list[ index ];

		/// <summary>
		/// 引数の数を返します
		/// </summary>
		public int Count => m_list.Count;

		/// <summary>
		/// 指定された文字列のコレクションから引数のリストを作成するコンストラクタ
		/// </summary>
		public CommandArguments( params string[] collection )
		{
			m_list = new List<string>( collection );
		}

		/// <summary>
		/// 指定された文字列のコレクションから引数のリストを作成するコンストラクタ
		/// </summary>
		public CommandArguments( IEnumerable<string> collection )
		{
			m_list = new List<string>( collection );
		}

		/// <summary>
		/// 指定されたインデックスの引数を string 型のまま返します
		/// </summary>
		public string ToStr( int index, string defaultVal = "" )
		{
			return ElementAtOrDefault( index, defaultVal );
		}

		/// <summary>
		/// 指定されたインデックスの引数を int 型に変換して返します
		/// </summary>
		public int ToInt( int index, string defaultVal = "0" )
		{
			return int.Parse( ElementAtOrDefault( index, defaultVal ) );
		}

		/// <summary>
		/// 指定されたインデックスの引数を float 型に変換して返します
		/// </summary>
		public float ToFloat( int index, string defaultVal = "0" )
		{
			return float.Parse( ElementAtOrDefault( index, defaultVal ) );
		}

		/// <summary>
		/// 指定されたインデックスの引数を bool 型に変換して返します
		/// </summary>
		public bool ToBool( int index, string defaultVal = "0" )
		{
			return int.Parse( ElementAtOrDefault( index, defaultVal ) ) != 0;
		}

		/// <summary>
		/// 指定されたインデックスの引数を列挙型に変換して返します
		/// </summary>
		public T ToEnum<T>( int index, string defaultVal = "0" )
		{
			return ( T ) Enum.Parse( typeof( T ), ElementAtOrDefault( index, defaultVal ) );
		}

		/// <summary>
		/// 現在のオブジェクトを表す文字列を返します
		/// </summary>
		public override string ToString()
		{
			return string.Join( ",", m_list.ToArray() );
		}

		/// <summary>
		/// シーケンス内の指定されたインデックス位置にある要素を返します。インデックスが範囲外の場合は既定値を返します。
		/// </summary>
		private string ElementAtOrDefault( int index, string defaultVal = "0" )
		{
			if ( 0 <= index && index < m_list.Count )
			{
				return m_list[ index ];
			}

			return defaultVal; // デフォルト値を返す
		}
	}
}