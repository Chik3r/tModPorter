#nullable enable
using System;

namespace tModPorter; 

public struct Result {
	public readonly bool Success;
	public readonly Exception? ErrorCause;

	public Result(bool success, Exception? errorCause) {
		Success = success;
		ErrorCause = errorCause;
	}
}