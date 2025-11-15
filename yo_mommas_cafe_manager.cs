using System;
using System.Collections.Generic;
using System.IO; // Required for File I/O operations 

// Namespace acts as a container for code.
namespace CafeManager
{
    // The static class holds all methods and data — replaces Python's global scope.
    public static class CafeManager
    {
        // ====================================================================
        // 1. STATIC FIELDS (Constants and Global Data)
        // ====================================================================

        public const string CAFE_NAME = "Campus Café";
        public const double TAX_RATE = 0.095;
        public const string DISCOUNT_CODE = "STUDENT10";
        public const double DISCOUNT_RATE = 0.10;

        // C# List<T> requires specifying the data type (string, double, int).
        private static List<string> item_names = new List<string>();
        private static List<double> item_prices = new List<double>();
        private static List<int> item_quantities = new List<int>();

        // 'bool' is the C# type for True/False (lowercase values).
        private static bool discount_applied_this_session = false;

        // ====================================================================
        // 2. ENTRY POINT (The Traditional Main Method)
        // ====================================================================

        // This is the required start method when "No Top-Level Statements" is selected.
        public static void Main(string[] args)
        {
            load_cart(false); // Attempt to load cart on startup (silent)
            main_menu();      // Starts the core application loop.
        }

        // ====================================================================
        // 3. UTILITY METHODS (Core Logic)
        // ====================================================================

        public static void show_banner()
        {
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"{CAFE_NAME} - Tax Rate: {TAX_RATE:F3}");
            Console.WriteLine(new string('=', 40));
        }

        public static double compute_subtotal()
        {
            double subtotal = 0.0;
            for (int i = 0; i < item_names.Count; i++)
            {
                subtotal += item_prices[i] * item_quantities[i];
            }
            return subtotal;
        }

        public static double compute_tax(double subtotal, double tax_rate)
        {
            return subtotal * tax_rate;
        }

        public static void add_item(string name, double price, int qty)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Error: Item name cannot be empty, idiot!");
                return;
            }
            if (price < 0.0)
            {
                Console.WriteLine("Error: Item price must be positive, dumbdumb!");
                return;
            }
            if (qty <= 0)
            {
                Console.WriteLine("Error: Item quantity must be at least 1, you mouthbreather!");
                return;
            }

            item_names.Add(name);
            item_prices.Add(price);
            item_quantities.Add(qty);
            Console.WriteLine($"Added {name} x {qty} @ ${price:F2}");
        }

        public static void remove_item(int index)
        {
            int actual_index = index - 1;

            if (actual_index >= 0 && actual_index < item_names.Count)
            {
                string removed_name = item_names[actual_index];
                item_names.RemoveAt(actual_index);
                item_prices.RemoveAt(actual_index);
                item_quantities.RemoveAt(actual_index);

                Console.WriteLine($"Removed '{removed_name}' from the cart.");
            }
            else
            {
                Console.WriteLine("Error: Invalid item index, idiot!");
            }
        }

        public static void clear_cart()
        {
            item_names.Clear();
            item_prices.Clear();
            item_quantities.Clear();

            discount_applied_this_session = false;
            Console.WriteLine("\nYour cart has been cleared.");
        }

        public static double apply_discount(double subtotal, string code)
        {
            if (code == DISCOUNT_CODE && !discount_applied_this_session)
            {
                discount_applied_this_session = true;
                Console.WriteLine("STUDENT10 discount applied! You saved 10 percent, cheapskate turdburglar!");
                return subtotal * DISCOUNT_RATE;
            }
            else if (code == DISCOUNT_CODE && discount_applied_this_session)
            {
                Console.WriteLine("Discount code has already been used this session! The FBI has been notified.");
                return 0.0;
            }
            else
            {
                if (!string.IsNullOrEmpty(code))
                {
                    Console.WriteLine("Error: Invalid discount code, cheapskate turdburglar!");
                }
                return 0.0;
            }
        }

        public static void apply_discount_from_menu()
        {
            if (item_names.Count == 0)
            {
                Console.WriteLine("Your cart is empty. Add items before applying a discount. Holy Satan, you are simple!");
                return;
            }

            if (discount_applied_this_session)
            {
                Console.WriteLine("A discount has already been applied to this order, the FBI has been notified!");
                return;
            }

            double subtotal = compute_subtotal();

            Console.Write("Enter discount code, you cheapskate: ");
            string user_code = Console.ReadLine();

            double discount_amount = apply_discount(subtotal, user_code);

            if (discount_amount > 0.0)
            {
                double new_subtotal = subtotal - discount_amount;
                double tax_amount = compute_tax(new_subtotal, TAX_RATE);
                double new_total = new_subtotal + tax_amount;

                Console.WriteLine("\n--- Discount Preview ---");
                Console.WriteLine($"You saved:      ${discount_amount:F2}");
                Console.WriteLine($"New Subtotal:   ${new_subtotal:F2}");
                Console.WriteLine($"Estimated Tax:  ${tax_amount:F2}");
                Console.WriteLine($"Estimated Total:${new_total:F2}");
            }
        }

        public static void view_cart()
        {
            Console.WriteLine("\n--- Current Cart ---");

            if (item_names.Count == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            Console.WriteLine("{0,-4} {1,-20} {2,10} {3,5} {4,10}",
                              "No.", "Item", "Price", "Qty", "Total");
            Console.WriteLine(new string('-', 52));

            double subtotal = compute_subtotal();
            double most_expensive_price = -1.0;
            string most_expensive_name = "";

            for (int i = 0; i < item_names.Count; i++)
            {
                double price = item_prices[i];
                int qty = item_quantities[i];
                double line_total = price * qty;

                Console.WriteLine("{0,-4} {1,-20} ${2,9:F2} {3,5} ${4,9:F2}",
                                  i + 1, item_names[i], price, qty, line_total);

                if (price > most_expensive_price)
                {
                    most_expensive_price = price;
                    most_expensive_name = item_names[i];
                }
            }

            Console.WriteLine(new string('-', 52));
            double tax_amount = compute_tax(subtotal, TAX_RATE);
            double estimated_total = subtotal + tax_amount;

            Console.WriteLine("{0,-40} ${1,10:F2}", "Subtotal:", subtotal);
            Console.WriteLine("{0,-40} ${1,10:F2}", "Tax:", tax_amount);
            Console.WriteLine("{0,-40} ${1,10:F2}", "Estimated Total:", estimated_total);

            if (subtotal > 0)
            {
                double average_line_total = subtotal / item_names.Count;
                string most_expensive_item_str = $"{most_expensive_name} @ ${most_expensive_price:F2}";

                Console.WriteLine($"\nAverage Line Total: ${average_line_total:F2}");
                Console.WriteLine($"Most Expensive Item: {most_expensive_item_str}");
            }
        }

        public static void checkout()
        {
            if (item_names.Count == 0)
            {
                Console.WriteLine("Cannot check out, your cart is empty, cheapskate turdburglar!");
                return;
            }

            Console.WriteLine("\nCHECKOUT");
            double subtotal = compute_subtotal();
            double discount_amount = 0.0;

            if (discount_applied_this_session)
            {
                discount_amount = subtotal * DISCOUNT_RATE;
            }

            double subtotal_after_discount = subtotal - discount_amount;
            double tax_amount = compute_tax(subtotal_after_discount, TAX_RATE);
            double final_total = subtotal_after_discount + tax_amount;

            Console.WriteLine("\n--- RECEIPT ---");
            Console.WriteLine($"Subtotal:       ${subtotal:F2}");

            if (discount_amount > 0)
            {
                Console.WriteLine($"Discount:      -${discount_amount:F2}");
            }

            Console.WriteLine($"Tax:            ${tax_amount:F2}");
            Console.WriteLine("-------------------");
            Console.WriteLine($"TOTAL:          ${final_total:F2}");
            Console.WriteLine($"\nThank you for visiting {CAFE_NAME}!");
        }

        // ====================================================================
        // 4. FILE I/O METHODS (New)
        // ====================================================================

        public static void save_cart(string filename = "cart.csv")
        {
            if (item_names.Count == 0)
            {
                Console.WriteLine("Cannot save: the cart is empty.");
                return;
            }

            try
            {
                // StreamWriter = writing text to a file. 'using' ensures the file is closed.
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine("Name,Price,Quantity"); // Header row

                    for (int i = 0; i < item_names.Count; i++)
                    {
                        // Write data as comma-separated values (CSV)
                        writer.WriteLine($"{item_names[i]},{item_prices[i]},{item_quantities[i]}");
                    }
                }

                // Save the discount status separately for simplicity.
                File.WriteAllText("discount_status.txt", discount_applied_this_session.ToString());

                Console.WriteLine($"\nCart successfully saved to {filename} and status saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError saving cart: {ex.Message}");
            }
        }
        
        // Silent flag = suppress console output when loading on startup.
        public static void load_cart(bool silent = false, string filename = "cart.csv")
        {
            clear_cart(); // Always clear current cart before loading new data.

            if (!File.Exists(filename))
            {
                if (!silent)
                    Console.WriteLine("\nNo saved cart found.");
                return;
            }

            try
            {
                // StreamReader = reading text from a file.
                using (StreamReader reader = new StreamReader(filename))
                {
                    // Skip the header row
                    if (!reader.EndOfStream)
                    {
                        reader.ReadLine(); 
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null) // Read line by line
                    {
                        string[] parts = line.Split(',');

                        if (parts.Length == 3)
                        {
                            // Parse strings back into their respective types (double and int).
                            string name = parts[0];
                            double price = double.Parse(parts[1]);
                            int qty = int.Parse(parts[2]);

                            // Add item data back to the parallel lists.
                            item_names.Add(name);
                            item_prices.Add(price);
                            item_quantities.Add(qty);
                        }
                    }
                } 
                
                // Load discount status
                if (File.Exists("discount_status.txt"))
                {
                    string status_text = File.ReadAllText("discount_status.txt");
                    discount_applied_this_session = bool.Parse(status_text);
                }

                if (!silent)
                    Console.WriteLine($"\nCart successfully loaded. {item_names.Count} items restored.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError loading cart. Data may be corrupt: {ex.Message}");
            }
        }


        // ====================================================================
        // 5. MAIN PROGRAM LOOP (User Interface)
        // ====================================================================

        public static void main_menu()
        {
            show_banner();

            while (true)
            {
                Console.WriteLine("\n1) Add item");
                Console.WriteLine("2) View cart");
                Console.WriteLine("3) Remove item");
                Console.WriteLine("4) Apply Discount");
                Console.WriteLine("5) Clear Cart");
                Console.WriteLine("6) Checkout");
                Console.WriteLine("7) Quit");
                Console.WriteLine("8) Save Cart");   // New Option
                Console.WriteLine("9) Load Cart");   // New Option

                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Item name: ");
                    string item_name = Console.ReadLine();

                    try
                    {
                        Console.Write("Item price: ");
                        double item_price = double.Parse(Console.ReadLine());

                        Console.Write("Quantity: ");
                        int item_qty = int.Parse(Console.ReadLine());

                        add_item(item_name, item_price, item_qty);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Price and quantity must be numbers.");
                    }
                }

                else if (choice == "2")
                {
                    view_cart();
                }

                else if (choice == "3")
                {
                    if (item_names.Count == 0)
                    {
                        Console.WriteLine("Your cart is empty. Nothing to remove.");
                        continue;
                    }
                    try
                    {
                        Console.Write("Enter item number to remove: ");
                        int item_num = int.Parse(Console.ReadLine());
                        remove_item(item_num);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number!");
                    }
                }

                else if (choice == "4")
                {
                    apply_discount_from_menu();
                }

                else if (choice == "5")
                {
                    clear_cart();
                }

                else if (choice == "6")
                {
                    checkout();
                    break;
                }

                else if (choice == "7")
                {
                    Console.WriteLine($"Goodbye! Thank you for using the {CAFE_NAME} manager.");
                    break;
                }
                
                // New File I/O Options
                else if (choice == "8")
                {
                    save_cart();
                }
                
                else if (choice == "9")
                {
                    load_cart();
                }

                else
                {
                    Console.WriteLine("Invalid choice. Please enter a number from 1 to 9.");
                }
            }
        }
    }
}