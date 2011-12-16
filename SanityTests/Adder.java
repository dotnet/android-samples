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

	public interface Progress {
		void onAdd (int[] values, int currentIndex, int currentSum);
	}

	public static class DefaultProgress implements Progress {
		public void onAdd (int[] values, int currentIndex, int currentSum)
		{
		}
	}

	public static int sum (Adder adder, Progress progress, int... values)
	{
		if (values == null || values.length == 0) {
			int r = adder.add (0, 0);
			progress.onAdd (values, -1, r);
			return r;
		}

		int s = 0;
		for (int i = 0; i < values.length; ++i) {
			s = adder.add (values [i], s);
			progress.onAdd (values, i, s);
		}

		return s;
	}
}

