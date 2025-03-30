//
//  PantryApp.swift
//  Pantry
//
//  Created by Matthias Röhricht on 29.12.24.
//

import SwiftUI
import SwiftData

@main
struct PantryApp: App {

    var sharedModelContainer: ModelContainer = {
        let schema = Schema([
            Recipe.self,
            Ingredient.self,
        ])
        let modelConfiguration = ModelConfiguration(schema: schema, isStoredInMemoryOnly: false, cloudKitDatabase: .private("iCloud.de.reedsoft.Pantry"))
        //let modelConfiguration = ModelConfiguration(schema: schema, isStoredInMemoryOnly: false)

        do {
            return try ModelContainer(for: schema, configurations: [modelConfiguration])
        } catch {
            fatalError("Could not create ModelContainer: \(error)")
        }
    }()

    var body: some Scene {
        WindowGroup {
            ContentView().modelContainer(for: [Recipe.self, Ingredient.self])
        }
        .modelContainer(sharedModelContainer)
    }
}
