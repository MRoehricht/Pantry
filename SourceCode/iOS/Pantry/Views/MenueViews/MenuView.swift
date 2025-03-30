//
//  Untitled.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI

struct MenuView: View {
    var body: some View {
        NavigationView {
            List {
                NavigationLink(destination: ProfileView()) {
                    Text("Profil anzeigen")
                }
                Button(action: {
                    // Füge hier die Logout-Funktionalität hinzu
                    print("Ausloggen")
                }) {
                    Text("Ausloggen")
                        .foregroundColor(.red)
                }
            }
            .navigationTitle("Menü")
        }
    }
}

struct ProfileView: View {
    var body: some View {
        Text("Profilseite")
            .navigationTitle("Profil")
    }
}

#Preview {
    MenuView()
}
