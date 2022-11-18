using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BeSwarm.WebApi.Models;

public static class StatusAction
{
	public const int Ok = StatusCodes.Status200OK;
	public const int Notfound = StatusCodes.Status404NotFound;
	public const int Internalerror = StatusCodes.Status500InternalServerError;
	public const int Logicalerror = StatusCodes.Status400BadRequest;
	public const int Unauthorized = StatusCodes.Status401Unauthorized;
	public const int Forbidden = StatusCodes.Status403Forbidden;
}
public interface IResultAction
{
	int Status { get; set; }
	InternalError Error { get; set; } 
}
public class ResultAction<T> : IResultAction
{
	public T Datas { get; set; }
	public int Status { get; set; }
	public InternalError Error { get; set; } = new();

	public ResultAction()
	{
		Status = StatusAction.Ok;
		var t = typeof(T);
		var name = t.Name;
		if (t.Name != "String" && t.Name != "Object" && !t.IsInterface) Datas = (T)Activator.CreateInstance(typeof(T));
		else Datas = default;
	}
	public bool IsOk
	{
		get
		{
			if (Status == StatusAction.Ok) return true;
			return false;
		}
	}

	public bool IsNotFound
	{
		get
		{
			if (Status == StatusAction.Notfound) return true;
			return false;
		}
	}

	public bool IsError
	{
		get
		{
			if (Status == StatusAction.Internalerror || Status == StatusAction.Logicalerror) return true;
			return false;
		}
	}


	public void SetError(InternalError error, int status)
	{
		Error = error;
		Status = status;
	}

	public void CopyStatusFrom( IResultAction src)
	{
		Status = src.Status;
		Error = src.Error;
	}
}

public class ResultAction : IResultAction
{
	public ResultAction()
	{
		Status = StatusAction.Ok;
	}

	public int Status { get; set; }
	public InternalError Error { get; set; } = new();

	public bool IsOk
	{
		get
		{
			if (Status == StatusAction.Ok) return true;
			return false;
		}
	}

	private bool IsNotFound
	{
		get
		{
			if (Status == StatusAction.Notfound) return true;
			return false;
		}
	}

	public bool IsError
	{
		get
		{
			if (Status != StatusAction.Ok && Status != StatusAction.Notfound) return true;
			return false;
		}
	}
	public void CopyStatusFrom(IResultAction src)
	{
		Status = src.Status;
		Error = src.Error;
	}
	public void SetError(InternalError error, int status)
	{
		Error = error;
		Status = status;
	}
}