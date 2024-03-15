int Fibonnaci(int num){
	int f1 = 0;
	int f2 = 1;
	for (int i = 0; i < num; i = i + 1){
		int result = f1 + f2;
		print(result);
		f1 = f2;
		f2 = result;
	}
	return f2;
}

int Max(int a, int b){
	if (a > b)
		return a;
	else
		return b;
}

int Min(int a, int b){
	if (a > b)
		return b;
	else
		return a;
}