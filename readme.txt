Kinetic of Ion Molecular Interaction Simulator (KIMI Sim) 

KIMI Sim is a numerical simulator of ion chemistry occurring in the various soft chemical ionisation mass-spectrometry (SCIMS) analytical techniques. The project was built to help understand complex ion-neutral interaction and simulate the ion and neutral number densities in a flow tube or a drift tube, but can be also used to simulate various other problems involving ion-molecular interaction. Shortly, KIMI Sim uses a simple GUI to create and manage set of ion-molecular reactions. The set of reaction is then numerically calculated using the classical Runge-Kutta technique, simulating Simulated data can be then downloaded or directly compared with the experimental results. 

This Readme file contains basic information about operation of KIMI Sim. Detailed information about the project can be found in my dissertation thesis. Video tutorial of how to use the KIMI Sim can be found in a link ‘not existing link’.

The ion-molecular reactions can be drafted in a graphical interface (reaction grid). First, ions and neutrals involved in the reactions must be created. This require definition of their names, masses and other parameters used in the simulation. The collection of all created species can be overview and managed in the Properties -> Resource manager. Reactions are then created by placing selected ion species on the reaction grid and connecting them by a reaction line. Reaction lines will appear between a selected ion and another grid slot or ion by dragging a mouse between them. Reaction parameters as rate constant and neutrals can be then specified and reaction can be accepted. Collection of ions and accepted reactions will be remembered for later use. Also, the reaction schemas can be saved for later use. When all ions on the reaction grid are properly connected, the simulation window can be open by button Proceed.

Main physical simulation parameters can be set up in Properties -> Calculation properties. The setup of the physical parameters can be also changed while the simulation window is open. 

The simulation window (reaction review) allows to modify initial concentrations of each species as well as rate constants of individual reactions. Simulation can be initiated by pressing Calculate button. Simulation uses the initial ion parameters and physical parameters for numerical calculation of differential equations, formed from draw ion-molecular reactions. Differential equations are calculated using a classical Runge-Kutta method along the reactor, following the setting provided in Calculation setting. The simulated ion distribution is then shown on the screen and can be exported. User is also able to investigate influence of a changing concentration of one ion or neutral species by selecting such an option in Reaction review. User is also able to investigate influence of static electric field. Such an option can be selected in Reaction review for a single one or a range of reduced electric field values. All of those additional options, software will execute individual simulation. Results can be reviewed, managed and downloaded in the Data plot tab.  

All executed simulations are temporarily saved and can be selected in Data management tab. After the selection, data from the particular run are restored and can be overviewed or downloaded.  

The KIMI Sim also allows to insert experimental data previously obtained by SFIT-MS, PTR-MS of SIFDT-MS experiments. The format of imported data was specified for Profile 3 SIFT-MS apparatus. 

Disclaimer

This project was created as a part of the European Union’s Horizon 2020 research and innovation programme. All the work in freely accessible and public, following the research share policy of EU founded projects. The project was developed in order to simulate chemical ionisation processes, occurring in the SCIMS techniques. The results can be used to describe those processes. The authors of the projects are happy share the project, but are not responsible for a poor quality of results if the project is not used properly. 

Acknowledgment
This project has received funding from the European Union’s Horizon 2020 research and innovation programme under the Marie Sk³odowska-Curie grant agreement no. 674911.


