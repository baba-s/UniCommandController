using System;

namespace UniCommandController
{
	/// <summary>
	/// コマンドの基底クラス
	/// </summary>
	public abstract class CommandBase :
		ICommand,
		IDisposable
	{
		/// <summary>
		/// コマンドが終了したかどうかを返します
		/// </summary>
		public virtual bool IsEnd { get => true; protected set { } }

		/// <summary>
		/// コマンドを初期化するときに呼び出されます
		/// </summary>
		public virtual void Start()
		{
			DoStart();
		}

		/// <summary>
		/// 派生クラスでコマンドの初期化処理を記述します
		/// </summary>
		protected virtual void DoStart()
		{
		}

		/// <summary>
		/// コマンドを更新するときに呼び出されます
		/// </summary>
		public virtual void Update()
		{
			DoUpdate();
		}

		/// <summary>
		/// 派生クラスでコマンドの更新処理を記述します
		/// </summary>
		protected virtual void DoUpdate()
		{
		}

		/// <summary>
		/// コマンドを削除するときに呼び出されます
		/// </summary>
		public virtual void Dispose()
		{
			DoDispose();
		}

		/// <summary>
		/// 派生クラスでコマンドの削除処理を記述します
		/// </summary>
		protected virtual void DoDispose()
		{
		}
	}
}