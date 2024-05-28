# FuelTracker

## Overview
FuelTracker is an Android application built with Xamarin.Android using C#.Net and SQLite as the database. The project is developed in Visual Studio 2022. The primary purpose of this application is to help users capture fuel fill details, calculate mileage, and determine the cost per kilometer. 

## Features
- **Fuel Fill Details**: Record the amount of fuel filled, money spent, and the current odometer reading. Users can also upload or capture images for reference.
- **Full Tank Assumption**: The application assumes that the tank is filled to full capacity every time.
- **Configuration**: Set your bike's tank capacity through the configuration page.
- **Mileage Calculation**: Automatically calculates mileage (km per liter) based on the fuel filled and distance traveled since the last fill-up.
- **Cost Calculation**: Determines the cost per kilometer based on the fuel price and mileage.
- **Fuel Availability Estimator**: Enter the current odometer value to estimate the remaining fuel and expected travel distance using the last calculated mileage.

## Installation
1. **Clone the repository**
    ```bash
    git clone https://github.com/ekalaivan92/Fuel-Monitor.git
    ```
2. **Open the project in Visual Studio 2022**
3. **Restore NuGet packages**
4. **Build the project**

## Usage
1. **Configuration**: 
   - Open the configuration page.
   - Set your bike’s tank capacity.

2. **Recording Fuel Fill Details**:
   - Enter the amount of fuel filled, the total cost, and the current odometer reading.
   - Optionally, upload or capture images of the fuel receipt or odometer.
   
3. **Calculating Mileage and Cost**:
   - The app will automatically calculate the mileage based on the difference between the current and previous odometer readings and the amount of fuel filled.
   - It will also compute the cost per kilometer.

4. **Fuel Availability Estimation**:
   - Go to the fuel availability calculator page.
   - Enter the current odometer reading.
   - The app will estimate the available fuel and expected travel distance based on the last calculated mileage.

## Example Calculation
If you filled 7 liters of fuel costing 700 rupees and your odometer reads 2300 km (previous reading was 2000 km), the application will perform the following calculations:
- **Mileage**: 
  \[
  \text{Mileage} = \frac{\text{Current ODO} - \text{Previous ODO}}{\text{Fuel Filled}} = \frac{2300 - 2000}{7} = 42 \text{ kmpl}
  \]
- **Cost per KM**:
  \[
  \text{Cost per KM} = \frac{\text{Total Cost}}{\text{Mileage}} = \frac{700}{42} \approx 16.67 \text{ rupees per km}
  \]

## Contributing
1. Fork the repository
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a pull request

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contact
For any inquiries or feedback, please contact [ekalaivan35@gmail.com](mailto:ekalaivan35@gmail.com).
