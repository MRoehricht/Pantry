//
//  AddRecipeView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI
import SwiftData

struct AddRecipeView: View {
    @Environment(\.modelContext) private var modelContext
    @Environment(\.presentationMode) private var presentationMode
    @State private var recipeName: String = ""
    @State private var ingredients: [Ingredient] = []
    @State private var ingredientName: String = ""
    @State private var ingredientQuantity: String = ""
    @State private var selectedImage: UIImage?
    @State private var showImagePicker: Bool = false

    var body: some View {
        Form {
            Section(header: Text("Rezeptname")) {
                TextField("Name", text: $recipeName)
            }
            Section(header: Text("Zutaten")) {
                ForEach(ingredients) { ingredient in
                    HStack {
                        Text(ingredient.name)
                        Spacer()
                        Text(ingredient.quantity)
                    }
                }
                HStack {
                    TextField("Zutat", text: $ingredientName)
                    TextField("Menge", text: $ingredientQuantity)
                    Button(action: addIngredient) {
                        Text("Hinzufügen")
                    }
                }
            }
            Section(header: Text("Bild")) {
                Button(action: {
                    showImagePicker = true
                }) {
                    Text("Bild auswählen")
                }
            }
            Button(action: addRecipe) {
                Text("Rezept speichern")
            }
        }
        .navigationTitle("Neues Rezept")
        .sheet(isPresented: $showImagePicker) {
            ImagePicker(image: $selectedImage)
        }
    }

    private func addIngredient() {
        let newIngredient = Ingredient(name: ingredientName, quantity: ingredientQuantity)
        ingredients.append(newIngredient)
        ingredientName = ""
        ingredientQuantity = ""
    }

    private func addRecipe() {
        let imageData = selectedImage?.jpegData(compressionQuality: 0.8)
        let newRecipe = Recipe(name: recipeName, ingredients: ingredients, imageData: imageData)
        modelContext.insert(newRecipe)
        recipeName = ""
        ingredients = []
        selectedImage = nil
        presentationMode.wrappedValue.dismiss()
    }
}

#Preview {
    AddRecipeView()
        .modelContainer(for: [Recipe.self, Ingredient.self])
}
