//
//  Untitled.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI
import SwiftData

struct RecipeView: View {
    @Environment(\.modelContext) private var modelContext
    @Query private var recipes: [Recipe]

    var body: some View {
        NavigationView {
            ScrollView {
                VStack {
                    ForEach(recipes) { recipe in
                        NavigationLink(destination: EditRecipeView(recipe: .constant(recipe))) {
                            RecipeCardView(recipe: recipe)
                        }
                    }
                }
            }
            .navigationTitle("Rezepte")
            .toolbar {
                ToolbarItem(placement: .navigationBarTrailing) {
                    NavigationLink(destination: AddRecipeView()) {
                        Text("Hinzufügen")
                    }
                }
            }
        }
    }
}

#Preview {
    RecipeView()
        .modelContainer(for: [Recipe.self, Ingredient.self])
}
