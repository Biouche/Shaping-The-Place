import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;

public class OffToObj {
	public static void main(String args[]) {
		String s = null;

		try {

			File dir = new File(
					".");
			File[] directoryListing = dir.listFiles();
			if (directoryListing != null) {
				for (File child : directoryListing) {
					String baseName = child.getName().split("\\.(?=[^\\.]+$)")[0];
					System.out.println(baseName);
					Process p = Runtime.getRuntime()
							.exec("meshlabserver -i " + baseName + ".off -o " + baseName + ".obj");

					BufferedReader stdInput = new BufferedReader(new InputStreamReader(p.getInputStream()));

					BufferedReader stdError = new BufferedReader(new InputStreamReader(p.getErrorStream()));

					// read the output from the command
					System.out.println("Here is the standard output of the command:\n");
					while ((s = stdInput.readLine()) != null) {
						System.out.println(s);
					}

					// read any errors from the attempted command
					System.out.println("Here is the standard error of the command (if any):\n");
					while ((s = stdError.readLine()) != null) {
						System.out.println(s);
					}
				}
			} else {

			}

		} catch (IOException e) {
			System.out.println("exception happened - here's what I know: ");
			e.printStackTrace();
			System.exit(-1);
		}
	}
}
