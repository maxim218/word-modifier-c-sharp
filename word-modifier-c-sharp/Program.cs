// libs
using System;
using System.Text;

// build app
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// sending page to user
app.Map("/page/main", appBuilder => {
	appBuilder.Run(async context => {
		const string message = "Visit url - " + "/page/main";
		app.Logger.LogInformation(message);
		const string pathToPage = "pages/main.html";
		await context.Response.SendFileAsync(pathToPage);
	});
});

// sending modified string to user
app.Map("/api/modify-string/get", appBuilder => {
	appBuilder.Run(async context => {
		string input = context.Request.Query["input"];
		string delta = context.Request.Query["delta"];
		string message = "Visit url - " + "/api/modify-string/get" + " with input - " + input + " and delta - " + delta;
		app.Logger.LogInformation(message);
		string modifiedString = new ModifierString(input, int.Parse(delta)).GetModifiedString().Trim();
		await context.Response.WriteAsync(modifiedString);
	});
});

// run app
app.Run();

// class for modifing string
class ModifierString {
	private string _s = string.Empty;
	private int _delta = 0;

	public ModifierString(string s, int delta) {
		_s = " " + s + " ";
		_delta = delta;
	}

	private string CalcSum(string numString) {
		int length = numString.Length;
		int num = int.Parse(numString);
		int sum = num + _delta;
		string sumString = "" + sum;
		if(sumString.Length == length) {
			return sumString;
		}
		while(sumString.Length != length) {
			sumString = sumString.Remove(0, 1);
		}
		return sumString;
	}

	public string GetModifiedString() {
		StringBuilder finalStringBuilder = new StringBuilder();

		StringBuilder numberSaveBuilder = new StringBuilder();

		bool numberStoring = false;

		for (int i = 0; i < _s.Length; i++) {
			char c = _s[i];

			if(!numberStoring) {
				if (Char.IsNumber(c)) {
					numberStoring = true;
					numberSaveBuilder.Append(c);
					continue;
				} else {
					finalStringBuilder.Append(c);
					continue;
				}
			}

			if (numberStoring) {
				if (Char.IsNumber(c)) {
					numberSaveBuilder.Append(c);
					continue;
				} else {
					numberStoring = false;
					string numString = numberSaveBuilder.ToString();
					numberSaveBuilder.Clear();
					finalStringBuilder.Append(CalcSum(numString));
					finalStringBuilder.Append(c);
					continue;
				}
			}
		}

		return finalStringBuilder.ToString();
	}
};
