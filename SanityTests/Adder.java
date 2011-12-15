package mono.android.test;

public class Adder {
	public int add (int a, int b)
	{
		return a + b;
	}

	public static int add (Adder self, int a, int b)
	{
		return self.add (a, b);
	}
}

