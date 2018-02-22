namespace UnityCommandController
{
	/// <summary>
	/// コマンドのインターフェイス
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// コマンドが終了したかどうかを返します
		/// </summary>
		bool IsEnd { get; }

		/// <summary>
		/// コマンドを初期化するときに呼び出されます
		/// </summary>
		void Start();

		/// <summary>
		/// コマンドを更新するときに呼び出されます
		/// </summary>
		void Update();

		/// <summary>
		/// コマンドを削除するときに呼び出されます
		/// </summary>
		void Dispose();
	}
}